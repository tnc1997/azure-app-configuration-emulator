export function download(name, bytes) {
    const element = document.createElement("a");

    element.download = name;
    element.href = "data:application/octet-stream;base64," + bytes;

    window.document.body.appendChild(element);

    element.click();

    window.document.body.removeChild(element);
}
