using System;
using System.Collections.Generic;
using FluentAssertions;
using In.ProjectEKA.OtpService.Clients;
using In.ProjectEKA.OtpService.Common;
using In.ProjectEKA.OtpService.Notification;
using In.ProjectEKA.OtpService.Otp;
using In.ProjectEKA.OtpServiceTest.Notification.Builder;
using Moq;
using Xunit;

namespace In.ProjectEKA.OtpServiceTest.Notification
{
    public class NotificationServiceTest
    {
        private readonly Mock<ISmsClient> notificationWebHandler = new Mock<ISmsClient>();
        private readonly NotificationProperties notificationProperties = new NotificationProperties("consent manager ID",
            new List<string>
            {
                "+91-9999999999",
                "+91-8888888888"
            });

        private readonly SmsServiceProperties smsServiceProperties = new SmsServiceProperties(String.Empty,
            String.Empty, String.Empty, String.Empty, String.Empty, 0, String.Empty, String.Empty); 
        private readonly NotificationService notificationService;

        public NotificationServiceTest()
        {
            notificationService = new NotificationService(notificationWebHandler.Object, notificationProperties, smsServiceProperties);
        }

        [Fact]
        private async void ReturnSuccessResponse()
        {
            var expectedResponse = new Response(ResponseType.Success, "Notification sent");
            notificationWebHandler.Setup(e => e.Send(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

            var response = await notificationService.SendNotification(TestBuilder.GenerateNotificationMessage());

            response.Should().Be(expectedResponse);
        }
        
        [Fact]
        private async void ReturnSuccessResponseForConsentManagerIdRecovered()
        {
            var expectedResponse = new Response(ResponseType.Success, "Notification sent");
            notificationWebHandler.Setup(e => e.Send(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

            var response = await notificationService.SendNotification(TestBuilder.GenerateNotificationMessageForConsentManagerIdRecovered());

            response.Should().Be(expectedResponse);
        }

        [Fact]
        private async void ReturnInternalServerError()
        {
            var expectedResponse = new Response(ResponseType.InternalServerError, "Internal server error");
            notificationWebHandler.Setup(e => e.Send(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

            var response = await notificationService.SendNotification(TestBuilder.GenerateNotificationMessage());

            response.ResponseType.Should().Be(expectedResponse.ResponseType);
        }
        
        [Fact]
        private async void ReturnInternalServerErrorForConsentManagerIdRecovered()
        {
            var expectedResponse = new Response(ResponseType.InternalServerError, "Internal server error");
            notificationWebHandler.Setup(e => e.Send(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResponse);

            var response = await notificationService.SendNotification(TestBuilder.GenerateNotificationMessageForConsentManagerIdRecovered());

            response.ResponseType.Should().Be(expectedResponse.ResponseType);
        }
        
        [Fact]
        private async void ReturnSuccessResponseIfMobileNumberIsWhiteListed()
        {
            var expectedResponse = new Response(ResponseType.Success, "Notification sent");

            var response = await notificationService.SendNotification(TestBuilder.GenerateNotificationMessageWithWhiTeListedMobileNo());

            response.ResponseType.Should().Be(expectedResponse.ResponseType);
            response.Message.Should().Be(expectedResponse.Message);
        }

    }
}