/**
 * HTMX Focus Restore
 * Preserves focus and cursor position on filter inputs that live inside
 * an HTMX-swapped container (e.g. tfoot search fields in index tables).
 *
 * Strategy: before the swap, snapshot the active element's placeholder and
 * cursor position; after the swap, find the matching input and restore them.
 */
(function () {
    let _placeholder = null;
    let _selectionStart = null;
    let _selectionEnd = null;

    document.body.addEventListener('htmx:beforeRequest', function (e) {
        const active = document.activeElement;
        const container = e.detail && e.detail.target;

        if (active && container && container.contains(active) &&
            (active.tagName === 'INPUT' || active.tagName === 'TEXTAREA')) {
            _placeholder = active.placeholder || null;
            _selectionStart = active.selectionStart;
            _selectionEnd = active.selectionEnd;
        } else {
            _placeholder = null;
        }
    });

    document.body.addEventListener('htmx:afterSwap', function (e) {
        if (!_placeholder) return;

        const container = e.detail && e.detail.target;
        if (!container) return;

        const input = container.querySelector(
            `input[placeholder="${CSS.escape(_placeholder)}"], textarea[placeholder="${CSS.escape(_placeholder)}"]`
        );

        if (input) {
            input.focus();
            try {
                input.setSelectionRange(_selectionStart, _selectionEnd);
            } catch (_) {
                // non-text inputs don't support setSelectionRange — ignore
            }
        }

        _placeholder = null;
        _selectionStart = null;
        _selectionEnd = null;
    });
})();
