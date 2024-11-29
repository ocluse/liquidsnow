using Microsoft.AspNetCore.Components.Forms;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that allows the user to upload files.
/// </summary>
public partial class UploadFile
{
    private bool _dragging;

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.UploadFile);
        classBuilder.AddIf(_dragging, ClassNameProvider.UploadFile_Dragging, DragClass);
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
    public SelectionMode Mode { get; set; } = SelectionMode.Multiple;

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
    public string? DragClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS applied to the drop zone.
    /// </summary>
    [Parameter]
    public string? DropZoneClass { get; set; }

    private void OnDragEnter()
    {
        _dragging = true;
    }

    private void OnDragLeave()
    {
        _dragging = false;
    }

    private Dictionary<string, object> GetInputAttributes()
    {
        var attributes = new Dictionary<string, object>();

        if (Mode == SelectionMode.Multiple)
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