﻿namespace Suzianna.Rest.Screenplay.Interactions
{
    public class Patch : HttpInteraction
    {
        private Patch() { }
        public static Patch DataAsJson(object content)
        {
            var patch = new Patch();
            patch.RequestBuilder.WithPatchVerb().WithContentAsJson(content);
            return patch;
        }
        public Patch To(string resource)
        {
            this.RequestBuilder.WithResourceName(resource);
            return this;
        }
      
    }
}
