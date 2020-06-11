﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Suzianna.Core.Screenplay;
using Suzianna.Rest.Screenplay.Abilities;
using Suzianna.Rest.Screenplay.Interactions;
using Suzianna.Rest.Tests.Unit.TestConstants;
using Suzianna.Rest.Tests.Unit.TestDoubles;
using Suzianna.Rest.Tests.Unit.TestUtils;
using Xunit;

namespace Suzianna.Rest.Tests.Unit.Screenplay
{
    public abstract class HttpInteractionTests
    {
        protected FakeHttpRequestSender Sender { get; } = new FakeHttpRequestSender();

        public static IEnumerable<object[]> GetUrlsWithRelativeResources()
        {
            return new List<object[]>
            {
                new object[] {"http://localhost:5050", "api/users", "http://localhost:5050/api/users"},
                new object[] {"http://localhost:5050", "/api/users", "http://localhost:5050/api/users"},
                new object[] {"http://localhost:5050/", "api/users", "http://localhost:5050/api/users"},
                new object[] {"http://localhost:5050/", "/api/users", "http://localhost:5050/api/users"}
            };
        }

        public static IEnumerable<object[]> GetUrlsWithAbsoluteResources()
        {
            return new List<object[]>
            {
                new object[]
                    {"http://localhost:5050", "http://localhost:5050/api/users", "http://localhost:5050/api/users"},
                new object[] { "http://localhost:5050", "http://localhost:5050/api/users?id=10", "http://localhost:5050/api/users?id=10"
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetUrlsWithRelativeResources))]
        [MemberData(nameof(GetUrlsWithAbsoluteResources))]
        public void should_send_request_to_correct_url(string baseUrl, string resource, string expectedUrl)
        {
            var juliet = Actor.Named(Names.Juliet).WhoCan(CallAnApi.At(baseUrl).With(Sender));
            juliet.AttemptsTo(GetHttpInteraction(resource));
            Sender.GetLastSentMessage().RequestUri.AbsoluteUri.Should().Be(expectedUrl);
        }

        protected abstract HttpMethod GetHttpMethod();
        protected abstract HttpInteraction GetHttpInteraction(string resource);

        [Fact]
        public void should_add_query_string_when_resource_have_query_string_in_it()
        {
            var baseUrl = "http://localhost:5050";
            var resourceName = "api/users?page=2";
            var expectedUrl = "http://localhost:5050/api/users?page=2";
            var juliet = Actor.Named(Names.Juliet).WhoCan(CallAnApi.At(baseUrl).With(Sender));

            juliet.AttemptsTo(GetHttpInteraction(resourceName));

            Sender.GetLastSentMessage().RequestUri.AbsoluteUri.Should().Be(expectedUrl);
        }

        [Fact]
        public void should_send_request_headers()
        {
            var actor = ActorFactory.CreateSomeActorWithApiCallAbility(Sender);

            actor.AttemptsTo(GetHttpInteraction(Urls.UsersApi)
                .WithHeader(HttpHeaders.Accept, MediaTypes.ApplicationXml));

            Sender.GetLastSentMessage().Headers.Accept.First().MediaType.Should().Be(MediaTypes.ApplicationXml);
        }

        [Fact]
        public void should_send_request_to_url_with_query_parameters()
        {
            var baseUrl = "http://localhost:5050";
            var resourceName = "api/users";
            var expectedUrl = "http://localhost:5050/api/users?UserId=2&LocationId=3";
            var juliet = Actor.Named(Names.Juliet).WhoCan(CallAnApi.At(baseUrl).With(Sender));

            juliet.AttemptsTo(GetHttpInteraction(resourceName)
                .WithQueryParameter("UserId", "2")
                .WithQueryParameter("LocationId", "3"));

            Sender.GetLastSentMessage().RequestUri.AbsoluteUri.Should().Be(expectedUrl);
        }

        [Fact]
        public void should_sent_correct_http_verb()
        {
            var actor = ActorFactory.CreateSomeActorWithApiCallAbility(Sender);

            actor.AttemptsTo(GetHttpInteraction(Urls.UsersApi));

            Sender.GetLastSentMessage().Method.Should().Be(GetHttpMethod());
        }

        [Fact]
        public void should_use_query_string_both_from_resource_name_and_added_query_strings()
        {
            var baseUrl = "http://localhost:5050";
            var resourceName = "api/users?page=2";
            var expectedUrl = "http://localhost:5050/api/users?page=2&UserId=2&LocationId=3";
            var juliet = Actor.Named(Names.Juliet).WhoCan(CallAnApi.At(baseUrl).With(Sender));

            juliet.AttemptsTo(GetHttpInteraction(resourceName)
                .WithQueryParameter("UserId", "2")
                .WithQueryParameter("LocationId", "3"));

            Sender.GetLastSentMessage().RequestUri.AbsoluteUri.Should().Be(expectedUrl);
        }
    }
}