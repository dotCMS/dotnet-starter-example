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

## Available Content Type Views

### Activity

Renders an activity card with image, title, description, tags, and a "Learn More" link.

### Banner

Renders a banner with background image, title, and optional button text and link.

### SimpleWidget

A basic widget that displays its title.

### YouTube

Renders a YouTube video embed with video information. Supports the following properties:

- `id`: The YouTube video ID (required for embedding)
- `author`: The creator of the video
- `length`: The duration of the video
- `thumbnailLarge`: URL to a large thumbnail image
- `thumbnailSmall`: URL to a small thumbnail image
- `titleImage`: Indicates which thumbnail to use (e.g., "thumbnailCustom")
- `thumbnailCustom`: A custom thumbnail identifier
- `hasLiveVersion`: Boolean indicating if there's a live version of the video

Example usage:

```html
<contentlet-renderer contentlet="youtubeContentlet"></contentlet-renderer>
```

## Usage in Templates

To use the `ContentletTagHelper` in your views:

```html
<contentlet-renderer contentlet="yourContentlet"></contentlet-renderer>
```

Where `yourContentlet` is an instance of `RazorPagesDotCMS.Models.Contentlet`.
