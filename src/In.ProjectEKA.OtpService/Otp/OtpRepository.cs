namespace In.ProjectEKA.OtpService.Otp
{
	using System;
	using System.Threading.Tasks;
	using Common;
	using Common.Logger;
	using Microsoft.EntityFrameworkCore;
	using Model;
	using Optional;

	public class OtpRepository : IOtpRepository
    {
        private readonly OtpContext otpContext;

        public OtpRepository(OtpContext otpContext)
        {
            this.otpContext = otpContext;
        }

        public async Task<Response> Save(string otp, string sessionId)
        {
            try
            {
                var otpRequest = new OtpRequest
                    {SessionId = sessionId, RequestedAt = DateTime.Now.ToUniversalTime(), OtpToken = otp};
                otpContext.OtpRequests.Add(otpRequest);
                await otpContext.SaveChangesAsync();
                return new Response(ResponseType.Success, "Otp Created");
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
                return new Response(ResponseType.InternalServerError, "OtpGeneration Saving failed");
            }
        }

        public async Task<Option<OtpRequest>> GetWith(string sessionId)
        {
            try
            {
                var otpRequest = await otpContext.OtpRequests
                    .Where(o => o.SessionId == sessionId)
                    .OrderByDescending(o => o.RequestedAt)
                    .FirstOrDefaultAsync();
                return otpRequest != null ? Option.Some(otpRequest) : Option.None<OtpRequest>();
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
                return Option.None<OtpRequest>();
            }
        }
    }
}