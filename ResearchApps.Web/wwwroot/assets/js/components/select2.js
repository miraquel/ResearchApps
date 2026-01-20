/**
 * Select2 Component - Classic jQuery Pattern
 * Reusable Select2 initializer using class selector and data-attributes
 * 
 * IMPORTANT: DO NOT use data-select2 attribute - it triggers Select2's internal
 * auto-initialization which causes corruption errors.
 * 
 * Usage in HTML:
 * <select class="form-select select2-ajax"
 *         data-ajax-url="/api/Items/cbo"
 *         data-placeholder="Select Item"
 *         data-id-field="itemId"
 *         data-text-field="itemName"
 *         data-paginated="true"              <!-- Optional: true for PagedListVm, false for plain list -->
 *         data-parent-id="#customerId"       <!-- Optional: selector for parent dropdown/field -->
 *         data-selected-id="5"
 *         data-selected-text="Item Name">
 * </select>
 * 
 * Then call: Select2Helper.initAll();
 */

(function ($) {
    'use strict';

    var Select2Helper = {
        /**
         * Initialize a single Select2 element with AJAX
         */
        init: function($element) {
            var ajaxUrl = $element.data('ajax-url');
            var placeholder = $element.data('placeholder') || 'Select an option';
            var idField = $element.data('id-field') || 'id';
            var textField = $element.data('text-field') || 'name';
            var allowClear = $element.data('allow-clear') === true;
            var paginated = $element.data('paginated') ?? false; // Default to false
            var selectedId = $element.data('selected-id');
            var selectedText = $element.data('selected-text');
            
            if (!ajaxUrl) {
                console.warn('No data-ajax-url found for Select2 element:', $element.attr('id'));
                return;
            }
            
            // Get anti-forgery token
            var token = $('input[name="__RequestVerificationToken"]').val();
            
            var config = {
                theme: 'bootstrap5',
                placeholder: placeholder,
                allowClear: allowClear,
                ajax: {
                    url: ajaxUrl,
                    dataType: 'json',
                    delay: 500,
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: function(params) {
                        var requestData;
                        
                        if (paginated) {
                            // PagedCboRequestVm format
                            requestData = {
                                Term: params.term || '',
                                PageNumber: params.page || 1,
                                PageSize: 10
                            };
                        } else {
                            // Plain list format (CboRequestVm)
                            requestData = {
                                Term: params.term || ''
                            };
                        }
                        
                        return requestData;
                    },
                    processResults: function(data) {
                        var items, hasMore;
                        
                        if (paginated) {
                            // PagedListVm format: { Items: [], HasNextPage: bool, ... }
                            items = data.Items || data.items || [];
                            hasMore = data.HasNextPage || data.hasNextPage || false;
                        } else {
                            // Plain array response
                            items = Array.isArray(data) ? data : (data.items || data.data || []);
                            hasMore = false; // No pagination for plain lists
                        }
                        
                        return {
                            results: items.map(function(item) {
                                return {
                                    id: item[idField],
                                    text: item[textField]
                                };
                            }),
                            pagination: {
                                more: hasMore
                            }
                        };
                    },
                    error: function(xhr) {
                        console.error('Select2 AJAX error:', xhr);
                    }
                }
            };
            
            // Initialize Select2
            $element.select2(config);
            
            // Set initial value if provided
            if (selectedId && selectedText) {
                var option = new Option(selectedText, selectedId, true, true);
                $element.append(option).trigger('change');
            }
        },
        
        /**
         * Initialize all elements with .select2-ajax class
         */
        initAll: function(container) {
            var $container = container ? $(container) : $(document);
            $container.find('.select2-ajax').each(function() {
                Select2Helper.init($(this));
            });
        },
        
        /**
         * Destroy Select2 on an element
         */
        destroy: function($element) {
            if ($element.hasClass('select2-hidden-accessible')) {
                $element.select2('destroy');
            }
        },
        
        /**
         * Set value programmatically
         */
        setValue: function($element, id, text) {
            if (id && text) {
                var option = new Option(text, id, true, true);
                $element.append(option).trigger('change');
            } else {
                $element.val(id).trigger('change');
            }
        },
        
        /**
         * Clear selection
         */
        clear: function($element) {
            $element.val(null).trigger('change');
        }
    };

    // Expose to window for global access
    window.Select2Helper = Select2Helper;

})(jQuery);
