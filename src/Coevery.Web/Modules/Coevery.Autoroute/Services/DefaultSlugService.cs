﻿using Coevery.ContentManagement;
using System.Text.RegularExpressions;
using Coevery.Utility.Extensions;
using System;

namespace Coevery.Autoroute.Services {

    public class DefaultSlugService : ISlugService {

        private readonly ISlugEventHandler _slugEventHandler;

        public DefaultSlugService(
            ISlugEventHandler slugEventHander
            ) {
                _slugEventHandler = slugEventHander;
        }

        public string Slugify(IContent content) {
            var metadata = content.ContentItem.ContentManager.GetItemMetadata(content);
            if (metadata == null || String.IsNullOrEmpty(metadata.DisplayText)) return null;
            var title = metadata.DisplayText.Trim();
            return Slugify(new FillSlugContext(content,title));
        }

        private string Slugify(FillSlugContext slugContext) {
            _slugEventHandler.FillingSlugFromTitle(slugContext);

            if (!slugContext.Adjusted) {

                var disallowed = new Regex(@"[/:?#\[\]@!$&'()*+,.;=\s\""\<\>\\\|%]+");

                var cleanedSlug = disallowed.Replace(slugContext.Title, "-").Trim('-','.');

                slugContext.Slug = Regex.Replace(cleanedSlug, @"\-{2,}", "-");

                if (slugContext.Slug.Length > 1000)
                    slugContext.Slug = slugContext.Slug.Substring(0, 1000).Trim('-', '.');

                slugContext.Slug = StringExtensions.RemoveDiacritics(slugContext.Slug.ToLower());
            }
            
            _slugEventHandler.FilledSlugFromTitle(slugContext);

            return slugContext.Slug;
        }

        public string Slugify(string text) {
            return Slugify(new FillSlugContext(null, text));
        }
       
    }
}
