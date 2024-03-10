export function close(id, result) {
    document.getElementById(id)?.close(result);
}

export function show(id) {
    document.getElementById(id)?.showModal();
}
