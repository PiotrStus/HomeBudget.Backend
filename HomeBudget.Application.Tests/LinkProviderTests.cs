using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Tests
{
    public class LinkProviderTests
    {
        [Theory]
        [InlineData("http://localhost:3000", "register/confirmAccount")]
        [InlineData("http://localhost:3000/", "/register/confirmAccount")]
        [InlineData("http://localhost:3000/", "register/confirmAccount/")]
        public void ShouldGenerateCorrectUrlWithVariousBaseUrls(string baseUrl, string confirmActionRelativeUrl)
        {
            var confirmationGuid = Guid.NewGuid();
            var settings = new LinkProviderSettings
            {
                BaseUrl = baseUrl,
                ConfirmActionRelativeUrl = confirmActionRelativeUrl
            };

            var linkProvider = new LinkProvider(Options.Create(settings));

            var expectedUrl = $"{baseUrl.TrimEnd('/')}/{confirmActionRelativeUrl.TrimStart('/')}?guid={confirmationGuid}";
            var result = linkProvider.GenerateConfirmationLink(confirmationGuid);

            Assert.Equal(expectedUrl, result);
        }

        [Theory]
        [InlineData("http://localhost:3000/", "register/confirmAccount")] 

        public void ShouldTrimBaseUrl(string baseUrl, string confirmActionRelativeUrl)
        {
            var confirmationGuid = Guid.NewGuid();
            var settings = new LinkProviderSettings
            {
                BaseUrl = baseUrl,
                ConfirmActionRelativeUrl = confirmActionRelativeUrl
            };

            var linkProvider = new LinkProvider(Options.Create(settings));

            var expectedUrl = $"{baseUrl.TrimEnd('/')}/{confirmActionRelativeUrl.TrimStart('/')}?guid={confirmationGuid}";
            var result = linkProvider.GenerateConfirmationLink(confirmationGuid);

            Assert.Equal(expectedUrl, result);
        }

        [Theory]
        [InlineData("http://localhost:3000", "/register/confirmAccount")]

        public void ShouldTrimConfirmActionRelativeUrl(string baseUrl, string confirmActionRelativeUrl)
        {
            var confirmationGuid = Guid.NewGuid();
            var settings = new LinkProviderSettings
            {
                BaseUrl = baseUrl,
                ConfirmActionRelativeUrl = confirmActionRelativeUrl
            };

            var linkProvider = new LinkProvider(Options.Create(settings));

            var expectedUrl = $"{baseUrl.TrimEnd('/')}/{confirmActionRelativeUrl.TrimStart('/')}?guid={confirmationGuid}";
            var result = linkProvider.GenerateConfirmationLink(confirmationGuid);

            Assert.Equal(expectedUrl, result);
        }

        [Theory]
        [InlineData(null, "register/confirmAccount")]

        public void ShouldThrowOnNullBaseUrl(string baseUrl, string confirmActionRelativeUrl)
        {
            var confirmationGuid = Guid.NewGuid();
            var settings = new LinkProviderSettings
            {
                BaseUrl = baseUrl,
                ConfirmActionRelativeUrl = confirmActionRelativeUrl
            };

            var linkProvider = new LinkProvider(Options.Create(settings));
            Assert.Throws<ArgumentNullException>(() => linkProvider.GenerateConfirmationLink(confirmationGuid));
            return;
        }

        [Theory]
        [InlineData("http://localhost:3000", "register/confirmAccount", "invalid-guid")]
        [InlineData("http://localhost:3000", "register/confirmAccount", "")]
        public void ShouldThrowOnInvalidGuid(string baseUrl, string confirmActionRelativeUrl, string guid)
        {
            var settings = new LinkProviderSettings
            {
                BaseUrl = baseUrl,
                ConfirmActionRelativeUrl = confirmActionRelativeUrl
            };

            var linkProvider = new LinkProvider(Options.Create(settings));

            Assert.Throws<FormatException>(() => linkProvider.GenerateConfirmationLink(Guid.Parse(guid)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldThrowOnNullOrEmptyConfirmActionRelativeUrl(string confirmActionRelativeUrl)
        {
            var settings = new LinkProviderSettings
            {
                BaseUrl = "https://example.com",
                ConfirmActionRelativeUrl = confirmActionRelativeUrl
            };

            var confirmationGuid = Guid.NewGuid();
            //var linkProvider = new LinkProvider(Options.Create(settings));

            var exception = Assert.Throws<ArgumentException>(() => new LinkProvider(Options.Create(settings)));
            Assert.Contains("ConfirmActionRelativeUrl cannot be null or empty.", exception.Message);
        }
    }
}