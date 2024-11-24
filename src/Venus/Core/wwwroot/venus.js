let watcherInitialized = false;

let watchedDropdown = null;

function closeDialog(dialog) {
    dialog.closeDialog();
}

function showDialog(dialog) {
    dialog.showModal();
}

function initializeDropdownWatcher() {
    if (watcherInitialized) {
        return;
    }
    watcherInitialized = true;

    document.addEventListener('click', function (event) {
        if (watchedDropdown && !watchedDropdown.contains(event.target)) {
            watchedDropdown.invokeMethodAsync('CloseDropdown');
            watchedDropdown = null;
        }
    });
}

function watchDropdown(dropdown) {
    if (watchedDropdown != dropdown) {
        unwatchDropdown(watchedDropdown);
    }
    watchedDropdown = dropdown;
}

function unwatchDropdown(dropdown) {
    if (watchedDropdown === dropdown) {
        watchedDropdown = null;
    }
}