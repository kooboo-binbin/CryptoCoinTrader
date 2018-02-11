<template>
    <div>
        <h1>arbitrage logs</h1>

        <p v-if="!arbitrages"><em>Loading...</em></p>
        <table class="table table-striped" v-if="arbitrages">
            <thead>
                <tr>
                    <th>Id</th>
                    <th>ObservationId</th>
                    <th>Volume</th>
                    <th>DateCreated</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in arbitrages">
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

    export default {
        data() {
            return {
                arbitrages: null,
                pagination: null,
            }
        },

        methods: {
            look(item) {
                console.log('look orders')
                console.log(item);
            },
            pageChange(p) {
                this.pagination.page = p;
                console.log('aa' + p);
            },
        },
        async created() {
            this.pagination = { page: 5, pageSize: 20, pageCount: 30, total: 600, hasNextPage: true, hasPreviousPage: true };
        }
    }
</script>
