function roleIndex() {
    return {
        currentPage: 1,
        sortBy: 'Name',
        sortAsc: true,
        isLoading: false,
        filters: {
            Name: '',
            Description: ''
        },

        init() {
            this.fetchList();
        },

        sort(column) {
            if (this.sortBy === column) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = column;
                this.sortAsc = true;
            }
            this.fetchList();
        },

        clearFilters() {
            this.filters = {
                Name: '',
                Description: ''
            };
            this.sortBy = 'Name';
            this.sortAsc = true;
            this.currentPage = 1;
            this.fetchList();
        },

        fetchList(page) {
            if (page) this.currentPage = page;
            this.isLoading = true;

            const params = new URLSearchParams();
            params.append('page', this.currentPage);
            params.append('sortBy', this.sortBy);
            params.append('sortAsc', this.sortAsc);

            for (const [key, value] of Object.entries(this.filters)) {
                if (value) params.append(`filters[${key}]`, value);
            }

            fetch(`/Admin/Roles/List?${params.toString()}`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
            .then(response => response.text())
            .then(html => {
                document.getElementById('role-list-container').innerHTML = html;
                this.isLoading = false;
            })
            .catch(error => {
                console.error('Error fetching role list:', error);
                this.isLoading = false;
            });
        }
    };
}
