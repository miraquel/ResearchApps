/**
 * Shared API error message extractor.
 * Prioritizes RFC 7807 ProblemDetails fields.
 * @param {Object} payload - Parsed JSON response body
 * @param {Response} response - Fetch Response object
 * @returns {string} Human-readable error message
 */
window.getApiErrorMessage = function(payload, response) {
    return payload?.detail
        || payload?.errors?.[0]
        || payload?.message
        || payload?.title
        || `Request failed (HTTP ${response?.status ?? 'unknown'})`;
};
