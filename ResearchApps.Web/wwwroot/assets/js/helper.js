const numberFormat = Intl.NumberFormat("en-US", { style: 'decimal', minimumFractionDigits: 0, maximumFractionDigits: 0 });
const percentFormat = Intl.NumberFormat('en-US', { style: 'percent', minimumFractionDigits: 1, maximumFractionDigits: 1 });
const percentFormat2Decimals = Intl.NumberFormat('en-US', { style: 'percent', minimumFractionDigits: 2, maximumFractionDigits: 2 });

function debounce(func, delay) {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), delay);
    };
}

// Render action buttons horizontally
// Each action: { href, icon (css class), text, class (optional), attrs (object) }
// options: { buttonClass (string), showText (boolean) }
window.renderActionDropdown = function(actions, options = {}) {
    const defaultButtonClass = options.buttonClass || 'btn btn-soft-secondary btn-sm';
    const showText = options.showText !== undefined ? options.showText : false;

    const buttons = (actions || []).map(a => {
        const btnClass = a.class ? `${defaultButtonClass} ${a.class}` : defaultButtonClass;
        const icon = a.icon ? `<i class="${a.icon}"></i>` : '';
        const text = showText && a.text ? ` ${a.text}` : '';
        const href = a.href || '#';
        const title = a.text || '';
        // allow additional attributes via attrs object (spread-like)
        const attrs = a.attrs ? Object.entries(a.attrs).map(([k,v]) => `${k}="${v}"`).join(' ') : '';
        return `<a href="${href}" class="${btnClass}" title="${title}" ${attrs}>${icon}${text}</a>`;
    }).join(' ');

    return `<div class="d-inline-flex gap-1">${buttons}</div>`;
};
