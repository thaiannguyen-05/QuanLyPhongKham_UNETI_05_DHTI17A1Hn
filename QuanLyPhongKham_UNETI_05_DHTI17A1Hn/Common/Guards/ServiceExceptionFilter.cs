using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Guards;

public sealed class ServiceExceptionFilter : IAsyncActionFilter
{
    private readonly IModelMetadataProvider _metadataProvider;

    public ServiceExceptionFilter(IModelMetadataProvider metadataProvider)
    {
        _metadataProvider = metadataProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var executed = await next();

        if (executed.Exception is null || executed.ExceptionHandled)
        {
            return;
        }

        switch (executed.Exception)
        {
            case KeyNotFoundException:
                executed.Result = new NotFoundResult();
                executed.ExceptionHandled = true;
                return;
            case ArgumentException ex:
                RejectWithForm(context, executed, ex.ParamName ?? string.Empty, ex.Message);
                return;
            case InvalidOperationException ex:
                RejectWithForm(context, executed, string.Empty, ex.Message);
                return;
        }
    }

    private void RejectWithForm(
        ActionExecutingContext context,
        ActionExecutedContext executed,
        string field,
        string message)
    {
        var model = context.ActionArguments.Values
            .FirstOrDefault(value => value is not null
                && value.GetType().IsClass
                && value is not string
                && value is not CancellationToken);

        if (model is null)
        {
            executed.Result = new BadRequestResult();
            executed.ExceptionHandled = true;
            return;
        }

        context.ModelState.AddModelError(field, message);
        executed.Result = new ViewResult
        {
            ViewName = context.RouteData.Values["action"]?.ToString(),
            ViewData = new ViewDataDictionary(_metadataProvider, context.ModelState)
            {
                Model = model
            }
        };
        executed.ExceptionHandled = true;
    }
}
