﻿using System.Net.Http;
using Suzianna.Rest.Screenplay.Interactions;

namespace Suzianna.Rest.Tests.Unit.Tests.Screenplay
{
    public class GetTests : HttpInteractionTests
    {
        protected override HttpMethod GetHttpMethod()=> HttpMethod.Get;

        protected override HttpInteraction GetHttpInteraction(string resource)
        {
            return Get.ResourceAt(resource);
        }
    }
}
