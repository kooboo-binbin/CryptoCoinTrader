<template>
    <div class="container-fluid">
        <div class="row">
            <div class="page-header col-lg-12 "><strong>Arbitrage logs</strong></div>
        </div>
        <div class="row">
            <div class="col-lg-12 box">
                <div class="form-inline">
                    <div class=" form-group">
                        <label for="observationName">Observation name</label>

                        <input type="text" class="form-control" id="observationName" v-model="observationName" v-on:keyup.enter="filter">
                    </div>
                    <div class=" form-group">
                        <label for="startDate">Start date</label>
                        <input type="date" class="form-control" id="startDate" v-model="startDate">
                    </div>
                    <div class=" form-group">
                        <label for="enddate">End date</label>
                        <input type="date" class="form-control" id="endDate" v-model="endDate">
                    </div>
                    <div class="form-group">
                        <button type="button" class="btn btn-primary" v-on:click="filter">Filter</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="box col-lg-12 table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Id</th>
                            <th>Observation name</th>
                            <th>Spread</th>
                            <th>Volume</th>
                            <th>Date created</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody v-if="!items">
                        <tr><td colspan="5"><em>Loading...</em></td></tr>
                    </tbody>
                    <tbody v-if="items">
                        <tr v-for="item in items">
                            <td>{{ item.id }}</td>
                            <td>{{ item.observationName }}</td>
                            <td>{{ item.spread }}</td>
                            <td>{{ item.volume }}</td>
                            <td>{{ item.dateCreated }}</td>
                            <td>  <router-link :to="{path:'/orders', query:{arbitrageId:item.id}}"><em title="find all orders" class="glyphicon glyphicon-tint"></em></router-link> </td>
                        </tr>
                    </tbody>
                </table>
            </div>

        </div>
        <pagination v-bind:pagination="pagination" v-on:pageChange="pageChange"></pagination>


    </div>

</template>
<script>
    var getData = async function (vue, changeRoute) {
        var data = {
            observationName: vue.observationName,
            startDate: vue.startDate,
            endDate: vue.endDate,
            page: vue.pagination.page,
            pageSize: vue.pagination.pageSize,
        };
        if (changeRoute) {
            vue.$router.push({ path: '/arbitrages', query: data });
        }
        let response = await vue.$http.get('api/arbitrages', { params: data });
        vue.items = response.data.items;
        vue.pagination = response.data.pagination;
    };

    export default {
        data() {
            return {
                observationName: null,
                startDate: null,
                endDate: null,
                items: null,
                pagination: { page: 1, pageSize: 20, pageCount: 1, total: 20, hasNextPage: false, hasPreviousPage: false },
            }
        },
        methods: {
            pageChange(p) {
                this.pagination.page = p;
                getData(this, true);
            },
            filter() {
                getData(this, true);
            }
        },
        async created() {
            this.observationName = this.$route.query.observationName;
            this.startDate = this.$route.query.startDate;
            this.endDate = this.$route.query.endDate;
            this.pagination.page = this.$route.query.page || 1;
            this.pagination.pageSize = this.$route.query.pageSize || 20;

            getData(this, false);
        }
    }
</script>
