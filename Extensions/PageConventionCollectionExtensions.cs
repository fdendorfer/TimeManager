using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace TimeManager.Extensions {
  public static class PageConventionCollectionExtensions {
    public static PageConventionCollection AuthorizeFolderOrPage(this PageConventionCollection conventions, string uri, string[] roles) {
      if (conventions == null)
        throw new ArgumentNullException(nameof(conventions));

      if (string.IsNullOrEmpty(uri))
        throw new ArgumentException("Argument cannot be null or empty.", nameof(uri));

      var policy = new AuthorizationPolicyBuilder().RequireRole(roles).Build();
      var authorizeFilter = new AuthorizeFilter(policy);
      conventions.AddPageApplicationModelConvention(uri, model => model.Filters.Add(authorizeFilter));
      return conventions;
    }

  }
}
