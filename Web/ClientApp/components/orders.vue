<template>
    <div class="container-fluid">
        <div class="row">
            <div class="page-header col-lg-12"><strong>Orders </strong></div>
        </div>

        <div class="row">
            <div class="col-lg-12 box">
                <div class="form-inline">

                    <div class=" form-group">
                        <label for="observationId">Observation name</label>

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
                            <th>Arbitrage id</th>
                            <th>Exchange name</th>
                            <!--<th>Order status</th>-->
                            <th>Volume</th>
                            <th>Currency pair</th>
                            <th>DateCreated</th>
                        </tr>
                    </thead>
                    <tbody v-if="!items">
                        <tr><td colspan="9"><em>Loading</em></td></tr>
                    </tbody>
                    <tbody v-if="items">
                        <tr v-for="item in items">
                            <td> <span v-bind:title="'remoeId:'+item.remoteId">{{ item.id }}</span></td>
                            <td>{{ item.observationName }}</td>
                            <td>{{ item.arbitrageId }}</td>
                            <td>{{ item.exchangeName }}</td>
                            <!--<td>{{ item.orderStatus }}</td>-->
                            <td>{{ item.volume }}</td>
                            <td>{{ item.currencyPair }}</td>
                            <td>{{ item.dateCreated }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <pagination v-bind:pagination="pagination" v-on:pageChange="pageChange"></pagination>

    </div>
</template>
<script>
    var getData = async function (vue) {
        var page = vue.pagination.page;
        var pageSize = vue.pagination.pageSize;
        var data = {
            observationName: vue.observationName,
            startDate: vue.startDate,
            endDate: vue.endDate,
            page: page,
            pageSize: pageSize
        };
        let response = await vue.$http.get('api/orders', { params: data });
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
                pagination: { page: 0, pageSize: 20, pageCount: 1, total: 20, hasNextPage: false, hasPreviousPage: false },
            }
        },
        methods: {

            pageChange(p) {
                this.pagination.page = p;
                getData(this);
            },
            filter() {
                getData(this);
            }
        },
        async created() {
            getData(this);
        }
    }
</script>
