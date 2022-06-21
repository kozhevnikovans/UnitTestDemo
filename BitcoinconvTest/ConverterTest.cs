using System;
using Xunit;
using Moq;
using Moq.Protected;
using Bitcoinconv;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinconvTest
{
    public class ConverterTest
    {
        private const string MOCK_RESPONSE_JSON = @"{""time"": {""updated"": ""Oct 15, 2020 22:55:00 UTC"",""updatedISO"": ""2020-10-15T22:55:00+00:00"",""updateduk"": ""Oct 15, 2020 at 23:55 BST""},""chartName"": ""Bitcoin"",""bpi"": {""USD"": {""code"": ""USD"",""symbol"": ""&#36;"",""rate"": ""11,486.5341"",""description"": ""United States Dollar"",""rate_float"": 11486.5341},""GBP"": {""code"": ""GBP"",""symbol"": ""&pound;"",""rate"": ""8,900.8693"",""description"": ""British Pound Sterling"",""rate_float"": 8900.8693},""EUR"": {""code"": ""EUR"",""symbol"": ""&euro;"",""rate"": ""9,809.3278"",""description"": ""Euro"",""rate_float"": 9809.3278}}}";

        private Converter mockConverter;

        public ConverterTest()
        {
            this.mockConverter= GetMockBitcoinConverterService();
        }
        private Converter GetMockBitcoinConverterService()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(MOCK_RESPONSE_JSON),
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            var converter = new Converter(httpClient);

            return converter;
        }

        [Theory]
        [InlineData("USD", 11486.5341)]
        [InlineData("GBP", 8900.8693)]
        [InlineData("EUR", 9809.3278)]
        public async void ReturnUSDExchangeRate(string currency,decimal result)
        {
            var exchangeRate = await mockConverter.GetExchangeRate(currency);

            Assert.Equal(result, exchangeRate);
        }
    }
}
