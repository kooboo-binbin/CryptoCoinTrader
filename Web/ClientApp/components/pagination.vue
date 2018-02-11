<template>
    <nav v-if="pagination" aria-label="Page navigation">
        <ul class="pagination">
            <li>
                <a href="#" v-bind:class="{disabled:!pagination.hasPreviousPage}" v-on:click="previous" aria-label="Previous">
                    <span aria-hidden="true">&laquo;</span>
                </a>
            </li>
            <li v-for="item in pages" v-bind:class="{active:item.active}"><a href="#" v-on:click="page(item.page)">{{item.page +1}}</a></li>
            <li>
                <a href="#" v-bind:class="{disabled:!pagination.hasNextPage}" v-on:click="next" aria-label="Next">
                    <span aria-hidden="true">&raquo;</span>
                </a>
            </li>
        </ul>
    </nav>
</template>
<script>
    var PageModel = function (active, page) {
        this.active = active;
        this.page = page;
    };

    export default {
        props: ['pagination'],
        data() {
            return {}
        },
        computed: {
            pages: function () {
                var temp = [];
                var current = this.pagination.page;
                var total = this.pagination.pageCount;
                for (var i = current - 4; i < this.pagination.page & i > 0; i++) {
                    temp.push(new PageModel(i == current, i));
                }
                for (var i = current; i < total & i < current + 4; i++) {
                    temp.push(new PageModel(i == current, i));
                }
                return temp;
            },
        },
        methods: {
            page: function (p) {
                this.$emit('pageChange', p);
            },
            next: function () {
                if (this.pagination.page < this.pagination.pageCount) {
                    this.$emit('pageChange', this.pagination.page - 1);
                }
            },
            previous: function () {
                if (this.pagination.page > 0) {
                    this.$emit('pageChange', this.pagination.page + 1);
                }
            }
        },

    }
</script>
