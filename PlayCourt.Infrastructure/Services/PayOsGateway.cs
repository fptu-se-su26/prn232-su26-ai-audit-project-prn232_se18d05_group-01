using System.Text.Json;
using Microsoft.Extensions.Options;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using PlayCourt.Application.Interfaces;
using PlayCourt.Application.Settings;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class PayOsGateway : IPayOsGateway
    {
        private readonly PayOSClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public PayOsGateway(IOptions<PayOsSettings> settings)
        {
            var value = settings.Value;
            _client = new PayOSClient(new PayOSOptions
            {
                ClientId = value.ClientId,
                ApiKey = value.ApiKey,
                ChecksumKey = value.ChecksumKey
            });
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<PayOsCreatePaymentLinkResult> CreatePaymentLinkAsync(
            PayOsCreatePaymentLinkRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.PaymentRequests.CreateAsync(new CreatePaymentLinkRequest
            {
                OrderCode = request.OrderCode,
                Amount = request.Amount,
                Description = request.Description,
                ReturnUrl = request.ReturnUrl,
                CancelUrl = request.CancelUrl
            });

            return new PayOsCreatePaymentLinkResult(response.CheckoutUrl, response.PaymentLinkId);
        }

        public async Task<PayOsPaymentLinkStatusResult> GetPaymentLinkInformationAsync(
            long orderCode,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.PaymentRequests.GetAsync(orderCode);
            var reference = response.Transactions?.FirstOrDefault()?.Reference;
            return new PayOsPaymentLinkStatusResult(
                response.OrderCode,
                response.Status.ToString(),
                reference,
                response.Id,
                response.AmountPaid,
                JsonSerializer.Serialize(response));
        }

        public async Task<PayOsWebhookResult> VerifyWebhookAsync(
            string webhookBody,
            CancellationToken cancellationToken = default)
        {
            var webhook = JsonSerializer.Deserialize<Webhook>(webhookBody, _jsonOptions)
                ?? throw new InvalidOperationException("Invalid PayOS webhook body.");
            var data = await _client.Webhooks.VerifyAsync(webhook);
            return new PayOsWebhookResult(
                data.OrderCode,
                data.Amount,
                data.Reference,
                data.PaymentLinkId,
                webhook.Success,
                webhookBody);
        }
    }
}
