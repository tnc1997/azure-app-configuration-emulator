export function addEventLeftClick(anchor, dotNetHelper) {
    document.getElementById(anchor)?.addEventListener("click", async function (event) {
        event.preventDefault();

        await dotNetHelper.invokeMethodAsync("OpenAsync", window.innerWidth, window.innerHeight, event.clientX, event.clientY);
    });
}

export function addEventRightClick(anchor, dotNetHelper) {
    document.getElementById(anchor)?.addEventListener("contextmenu", async function (event) {
        event.preventDefault();

        await dotNetHelper.invokeMethodAsync("OpenAsync", window.innerWidth, window.innerHeight, event.clientX, event.clientY);
    });
}