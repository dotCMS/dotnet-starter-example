# Content Type Views

This directory contains Razor views for rendering different content types from DotCMS. Each view corresponds to a specific content type and is used by the `ContentletTagHelper` to render contentlets based on their `ContentType` property.

## How It Works

The `ContentletTagHelper` looks for a view with a name matching the contentlet's `ContentType` property. For example, if a contentlet has `ContentType = "Banner"`, the tag helper will look for a view named `ContentTypes/Banner.cshtml`.

The view engine will search in the following locations:
1. `/Views/{ControllerName}/ContentTypes/Banner.cshtml`
2. `/Views/Shared/ContentTypes/Banner.cshtml`

For best compatibility, place your content type views in the `/Views/Shared/ContentTypes/` directory.

## Creating a New Content Type View

To create a view for a new content type:

1. Create a new `.cshtml` file in the `/Views/Shared/ContentTypes/` directory
2. Name it after the content type (e.g., `News.cshtml` for the "News" content type)
3. Start with the model directive: `@model RazorPagesDotCMS.Models.Contentlet`
4. Access standard properties directly (e.g., `Model.Title`)
5. Access custom fields through `Model.AdditionalProperties`

## Example: Accessing Custom Fields

Custom fields for a content type are available in the `AdditionalProperties` dictionary. Here's how to access them:

```csharp
@model RazorPagesDotCMS.Models.Contentlet
@{
    // Extract custom field values
    var customField = Model.AdditionalProperties?.TryGetValue("fieldName", out var fieldValue) == true && 
                     fieldValue.ValueKind == System.Text.Json.JsonValueKind.String
        ? fieldValue.GetString()
        : "";
}

<div>@customField</div>
```

## Fallback Behavior

If a view for a specific content type is not found, the `ContentletTagHelper` will fall back to a generic rendering that displays all properties of the contentlet.

## Usage in Templates

To use the `ContentletTagHelper` in your views:

```html
<contentlet-renderer contentlet="yourContentlet"></contentlet-renderer>
```

Where `yourContentlet` is an instance of `RazorPagesDotCMS.Models.Contentlet`.
