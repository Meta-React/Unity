mergeInto(LibraryManager.library, {
    SendPosition: function (x, y, z) {
        window.dispatchReactUnityEvent("SendPosition", x, y, z);
    },
});