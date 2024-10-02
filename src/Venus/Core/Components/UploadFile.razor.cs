using Microsoft.AspNetCore.Components.Forms;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that allows the user to upload files.
/// </summary>
public partial class UploadFile
{
    private bool _isDragging;

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("upload-file");
        classBuilder.AddIf(_isDragging, DragClass);
    }

    /// <summary>
    /// Gets or sets the inner content of the upload file component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the selected files are changed.
    /// </summary>
    [Parameter]
    public EventCallback<InputFileChangeEventArgs> OnFilesChanged { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the user can select multiple files.
    /// </summary>
    [Parameter]
    public bool Multiple { get; set; } = true;

    /// <summary>
    /// Gets or sets a value that determines the types of files that the user can select.
    /// </summary>
    [Parameter]
    public string? Accept { get; set; }

    /// <summary>
    /// Gets or sets the content to display on the drop zone of the component.
    /// </summary>
    [Parameter]
    public RenderFragment? MessageContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied when the user is dragging a file over the component.
    /// </summary>
    [Parameter]
    public string DragClass { get; set; } = "active";

    private void OnDragEnter()
    {
        _isDragging = true;
    }

    private void OnDragLeave()
    {
        _isDragging = false;
    }

    private Dictionary<string, object> GetInputAttributes()
    {
        var attributes = new Dictionary<string, object>();

        if (Multiple)
        {
            attributes.Add("multiple", "multiple");
        }
        if (Accept != null)
        {
            attributes.Add("accept", Accept);
        }

        return attributes;
    }
}