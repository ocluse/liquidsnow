let watcherInitialized = false;

let watchedDropdown = null;
let watchedDropdownId = null;

export function closeDialog(dialog) {
    dialog.close();
}

export function showDialog(dialog) {
    dialog.showModal();
}

export function showPopover(element) {
    element.showPopover();
}

export function initializeDropdownWatcher() {
    if (watcherInitialized) {
        return;
    }
    watcherInitialized = true;

    document.addEventListener('click', handleDocumentClick);
}

function handleDocumentClick(event) {
    const eventPath = event.composedPath();
    //the id is stored in the data-dropdown-id attribute

    if (watchedDropdownId == null || watchedDropdown == null) {
        return;
    }

    const clicked = eventPath.some(element =>
        element instanceof HTMLElement
        && element.getAttribute('data-dropdown-id') == watchedDropdownId);

    if (watchedDropdown != null && !clicked) {
        console.log("unwatching");
        watchedDropdown.invokeMethodAsync('CloseDropdown');
        watchedDropdown = null;
    }
}

export function watchDropdown(dropdown, dropdownId) {
    if (watchedDropdown != dropdown) {
        unwatchDropdown(watchedDropdown);
    }
    watchedDropdown = dropdown;
    watchedDropdownId = dropdownId;
}

export function unwatchDropdown(dropdown) {
    if (watchedDropdown === dropdown) {
        watchedDropdown = null;
        watchedDropdownId = null;
    }
}