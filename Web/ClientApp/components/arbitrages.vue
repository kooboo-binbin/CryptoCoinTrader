<template>

    <div>
        <h1>arbitrage logs</h1>

        <div class="row form-inline">
            <div class=" form-group">
                <label for="observationId">Observation id</label>

                <input type="text" class="form-control" id="observationId" v-model="observationId">
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
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Id</th>
                    <th>ObservationId</th>
                    <th>Volume</th>
                    <th>DateCreated</th>
                    <th></th>
                </tr>
            </thead>
            <tbody v-if="!items">
                <tr><td colspan="5"><em>Loading</em></td></tr>
            </tbody>
            <tbody v-if="items">
                <tr v-for="item in items">
                    <td>{{ item.id }}</td>
                    <td>{{ item.observationId }}</td>
                    <td>{{ item.volume }}</td>
                    <td>{{ item.dateCreated }}</td>
                    <td><href href="#" title="find all orders" v-on:click="look(item)"><em class="glyphicon glyphicon-piggy-bank"></em></href> </td>
                </tr>
            </tbody>
        </table>
        <pagination v-bind:pagination="pagination" v-on:pageChange="pageChange"></pagination>
    </div>


</template>
<script>
    var getData = async function (vue) {
        var page = vue.pagination.page;
        var pageSize = vue.pagination.pageSize;
        var data = {
            observationId: vue.observationId,
            startDate: vue.startDate,
            endDate: vue.endDate,
            page: page,
            pageSize: pageSize
        };
        let response = await vue.$http.get('api/arbitrages', { params: data });
        vue.items = response.data.items;
        vue.pagination = response.data.pagination;
    };

    export default {
        data() {
            return {
                observationId: null,
                startDate: null,
                endDate: null,

                items: null,
                pagination: { page: 1, pageSize: 20, pageCount: 1, total: 20, hasNextPage: false, hasPreviousPage: false },
            }
        },
        methods: {
            look(item) {
                console.log('look orders')
                console.log(item);
            },
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
