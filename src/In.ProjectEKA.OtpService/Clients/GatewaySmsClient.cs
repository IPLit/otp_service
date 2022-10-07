using System.Web;

namespace In.ProjectEKA.OtpService.Clients
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Net.Mime;
	using System.Text;
	using System.Threading.Tasks;
    using System.Runtime.Caching;
	using Common;
	using Common.Logger;
	using Microsoft.Net.Http.Headers;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Optional;
	using Otp;

	public class GatewaySmsClient : ISmsClient
    {
        private readonly SmsServiceProperties smsServiceProperties;
        private readonly HttpClient client;
        private MemoryCache cache;

        public GatewaySmsClient(SmsServiceProperties smsServiceProperties)
        {
            this.smsServiceProperties = smsServiceProperties;
            client = new HttpClient();
            cache = MemoryCache.Default;
        }

        public async Task<Response> Send(string phoneNumber, string message, string templateId)
        {
            var phoneNumberWithCountryCode = phoneNumber;
            // if (phoneNumber.Contains('-'))
            phoneNumberWithCountryCode = phoneNumber.Replace("+", string.Empty).Replace("-", String.Empty);
            if (phoneNumberWithCountryCode.Length == 12)
                phoneNumberWithCountryCode = phoneNumberWithCountryCode.Substring(2);

            try
            {
                var uriBuilder = new UriBuilder(smsServiceProperties.SmsApi);
                // uriBuilder.Port = -1;
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["username"] = smsServiceProperties.ClientId;
                query["password"] = smsServiceProperties.ClientSecret;
                query["message"] = message;
                query["destination"] = phoneNumberWithCountryCode;
                query["tempid"] = templateId;
                query["source"] = smsServiceProperties.Signature;
                query["entityid"] = smsServiceProperties.EntityId;
                query["type"] = "0";
                query["dlr"] = "1";
				Log.Error("smsQuery: "+query.ToString());

                uriBuilder.Query = query.ToString();

                var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());

                var response = await client
                    .SendAsync(request)
                    .ConfigureAwait(false);

                var contents = await response.Content.ReadAsStringAsync();
                Log.Information(contents);
                if (response.StatusCode == (HttpStatusCode) 200)
                    return new Response(ResponseType.Success, "Notification sent");
                Log.Error(response.StatusCode,response.Content);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
                return new Response(ResponseType.InternalServerError, "Unable to create otp message.");
            }

            return new Response(ResponseType.Success, "Error in sending notification");
        }
    }
}