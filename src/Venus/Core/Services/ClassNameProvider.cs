using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Services;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ClassNameProvider
{
    public string Avatar => "avatar";

    public string Button => "button";

    public virtual string BrandIcon => "brand-icon";

    public virtual string Card => "card";

    public string ComponentDisabled => "disabled";

    public string ComponentReadOnly => "readonly";

    public string ComponentActive => "active";

    public string ComponentHasValue => "has-value";

    public string ComponentError => "error";

    public string ComponentHeader=> "header";

    public string ComponentFooter => "footer";

    public string ComponentPrefix => "prefix";

    public string ComponentSuffix => "suffix";

    public string ComponentContent => "content";

    public string Card_Header=> "card_header";

    public string Card_Content => "card_content";

    public string Card_Footer => "card_footer";

    public string CheckBox => "checkbox";

    public string Checkbox_Checkmark => "checkbox_checkmark";

    public string ChipPicker => "chip-picker";

    public string ChipPicker_Item => "item";

    public string ChipPicker_ItemSelected => $"{ChipPicker_Item} {ComponentActive}";

    public string CollectionView => "collection-view";

    public string CollectionView_Item => "item";

    public string CollectionView_ItemsContainer => "items-container";

    public string CollectionView_Header => ComponentHeader;

    public string CollectionView_Footer => ComponentFooter;

    public string ListView => "list-view";

    public string LoadingSpinner => "loading-spinner";

    public string Paginator => "paginator";

    public string Paginator_Item => "paginator_item";

    public string Paginator_ItemActive => ComponentActive;

    public string Paginator_Next => "paginator_next";

    public string Paginator_Previous => "paginator_previous";

    public string Icon => "icon";

    public string Feather => "feather";

    public string Fluent => "fluent";

    public string FeatherIcon => $"{Icon} {Feather}";

    public string FluentIcon => $"{Icon} {Fluent}";

    public string IconButton => "icon-button";

    public string IconText => "icon-text";

    public string FeatherIconButton => $"{IconButton} {Feather}";

    public string FluentIconButton => $"{IconButton} {Fluent}";

    public string FormView => "form-view";

    public string Grid => "grid";

    public string GridView => "grid-view";

    public string InputDisabled => ComponentDisabled;

    public string InputReadOnly => ComponentReadOnly;

    public string InputError => ComponentError;

    public string InputHasValue => ComponentHasValue;

    public string ValidationSuccess => "validation-success";

    public string ValidationError => "validation-error";

    public string TextBox => "textbox";

    public string TextBox_Content => ComponentContent;

    public string TextBox_Header => ComponentHeader;

    public string TextBox_Prefix => ComponentPrefix;

    public string TextBox_Suffix => ComponentSuffix;

    public string TextBox_ValidationLabel => "textbox_validation";

    public string TextBox_Input => "textbox_input";

    public string ItemDetailContainer => "item-detail-container";

    public string LinkButton => "link";

    public string ListView_Container => "list";

    public string TableView => "table-view";

    public string TableView_Container => "table";

}
