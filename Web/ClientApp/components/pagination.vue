<template>


    <div class="row box">
        <div class="col-lg-5 page-details">
            showing {{recordStart}} to {{recordStart + pagination.pageSize}} of {{pagination.total}}
        </div>

        <nav class="col-lg-7 page-pagination" v-if="pagination" aria-label="Page navigation">
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

    </div>

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
                for (var i = current - 4; i < current; i++) {
                    if (i >= 0) {
                        temp.push(new PageModel(i == current, i));
                    }
                }
                for (var i = current; i < total & i < current + 4; i++) {
                    temp.push(new PageModel(i == current, i));
                }
                return temp;
            },
            recordStart: function () {
                return this.pagination.page * this.pagination.pageSize;
            }
        },
        methods: {
            page: function (p) {
                this.$emit('pageChange', p);
            },
            next: function () {
                if (this.pagination.page < this.pagination.pageCount) {
                    this.$emit('pageChange', this.pagination.page + 1);
                }
            },
            previous: function () {
                if (this.pagination.page > 0) {
                    this.$emit('pageChange', this.pagination.page - 1);
                }
            }
        },

    }
</script>
<style>
    .page-details {
        padding-top: 8px;
        white-space: nowrap;
    }

    .page-pagination {
        text-align: right;
    }

    .pagination {
        margin: 0;
    }
</style>
