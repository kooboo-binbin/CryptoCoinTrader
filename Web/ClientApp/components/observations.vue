<template>
    <div>
        <h1> Observations</h1>
        <button v-on:click="add">Add</button>
        <p v-if="!observations"><em>Loading...</em></p>
        <table class="table table-striped" v-if="observations">
            <thead>
                <tr>
                    <th>Buy exchange name</th>
                    <th>Sell exchange name</th>
                    <th>Currency pair</th>
                    <th>Running status</th>
                    <th>Spread type</th>
                    <th>Value / Percentage</th>
                    <th>Spread minimum</th>
                    <th>Per volume</th>
                    <th>Max </th>
                    <th>Availabe</th>
                    <th>Operations</th>
                </tr>

            </thead>
            <tbody>
                <tr v-for="item in observations">
                    <td>{{ item.buyExchangeName }}</td>
                    <td>{{ item.sellExchangeName }}</td>
                    <td>{{ item.currencyPair }}</td>
                    <td>{{ item.runningStatus }} <em class="glyphicon glyphicon-stop"></em> <em class="glyphicon glyphicon-play"></em></td>
                    <td>{{ item.spreadType }}</td>
                    <td>{{ item.spreadType=='value'? item.spreadValue:item.spreadPercentage }}</td>
                    <td>{{ item.spreadMinimumVolume }}</td>
                    <td>{{ item.perVolume }}</td>
                    <td>{{ item.maxVolume }}</td>
                    <td>{{ item.availabeVolume }}</td>
                    <td><em class="glyphicon glyphicon-edit" v-on:click="edit(item)"></em> <em v-on:click="remove(item)" class="glyphicon glyphicon-remove"></em> </td>
                </tr>
            </tbody>
        </table>
        <div id="observationModal" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Modal title</h4>
                    </div>
                    <div class="modal-body">
                        <p>One fine body&hellip;</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary">Save changes</button>
                    </div>
                </div><!-- /.modal-content -->
            </div><!-- /.modal-dialog -->
        </div><!-- /.modal -->
    </div>
</template>
<script>


    export default {
        data() {
            return {
                observations: null
            }
        },
        methods: {
            add: function () {
                //console.log(jQuery);
                console.log($("#observationModal").html());
               
                $('#observationModal').modal('show');
            },
            edit: function (item) {
                console.log(item);
                console.log('edit' + item.id);
            },
            remove: function (item) {
                console.log(item.id);
            }
        },
        async created() {
            try {
                let response = await this.$http.get('api/observations');
                console.log(response.data);
                this.observations = response.data;
            } catch (error) {
                console.log(error);
            }
        },
        mounted() {
            console.log($)
        }
    }

</script>
