using System;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;

public static class MvcHelpers
{
    public static string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = ControllerContext.RouteData.GetRequiredString("action");
        ViewDataDictionary ViewData = new ViewDataDictionary();
        TempDataDictionary TempData = new TempDataDictionary();
        ViewData.Model = model;

        using (StringWriter sw = new StringWriter())
        {
            ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
            ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
            viewResult.View.Render(viewContext, sw);

            return sw.GetStringBuilder().ToString();
        }

    }
}

public static class AriaHtmlHelperExtensions
{
    public static IHtmlString AriaTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
    {
        ModelMetadata metadata =
             ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        bool required = metadata.IsRequired;
        RouteValueDictionary attributes = new RouteValueDictionary();
        if (required)
            attributes.Add("aria-required", true);
        return html.TextBoxFor(expression, attributes);
    }

    public static IHtmlString AriaLabelFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
    {
        ModelMetadata metadata =
             ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        bool required = metadata.IsRequired;
        RouteValueDictionary attributes = new RouteValueDictionary();
        if (required)
            attributes.Add("aria-required", true);
        return html.LabelFor(expression, attributes);
    }
}