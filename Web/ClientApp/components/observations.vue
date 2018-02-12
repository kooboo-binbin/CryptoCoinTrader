<template>
    <div class="container-fluid">
        <div class="row page-header">
            <strong> Observations</strong>
        </div>
        <div class="row">
            <div class="box col-lg-12">
                <button v-on:click="add" class="btn btn-primary">Add</button>
            </div>
        </div>
        <div class="row">
            <div class="box col-lg-12 table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>From exchange name</th>
                            <th>To exchange name</th>
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
                    <tbody v-if="!items">
                        <tr><td colspan="11"><em>Loading</em></td></tr>
                    </tbody>
                    <tbody v-if="items">
                        <tr v-for="item in items">
                            <td>{{ item.fromExchangeName }}</td>
                            <td>{{ item.toExchangeName }}</td>
                            <td>{{ item.currencyPair }}</td>
                            <td>
                                {{ item.runningStatus }}

                            </td>
                            <td>{{ item.spreadType }}</td>
                            <td>{{ item.spreadType=='value'? item.spreadValue:item.spreadPercentage }}</td>
                            <td>{{ item.minimumVolume }}</td>
                            <td>{{ item.perVolume }}</td>
                            <td>{{ item.maxVolume }}</td>
                            <td>{{ item.availabeVolume }}</td>
                            <td><a href="#"><em class="glyphicon glyphicon-edit" v-on:click="edit(item)"></em></a> <a href="#"><em v-on:click="remove(item)" class="glyphicon glyphicon-remove"></em></a> </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div id="observationModal" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">{{ title }}</h4>
                    </div>
                    <div class="modal-body form-horizontal" v-if="observation">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">From exchange name</label>
                            <div class="col-sm-8 ">
                                <select class="form-control" v-model="observation.fromExchangeName">
                                    <option v-for="option in exchangeNames">
                                        {{option}}
                                    </option>
                                </select>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">To exchange name</label>
                            <div class="col-sm-8 ">
                                <select class="form-control" v-model="observation.toExchangeName">
                                    <option v-for="option in exchangeNames">
                                        {{option}}
                                    </option>
                                </select>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Currency pair</label>
                            <div class="col-sm-8 ">
                                <select class="form-control" v-model="observation.currencyPair">
                                    <option v-for="option in currencyPairs">
                                        {{option}}
                                    </option>
                                </select>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Running status</label>
                            <div class="col-sm-8 ">
                                <select class="form-control" v-model="observation.runningStatus">
                                    <option v-for="option in runningStatus">
                                        {{option}}
                                    </option>
                                </select>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Spread type</label>
                            <div class="col-sm-8 ">
                                <label><input type="radio" value="Value" v-model="observation.spreadType" />Value </label>
                                <br />
                                <label><input type="radio" value="Percentage" v-model="observation.spreadType" />Percentage </label>
                            </div>
                        </div>

                        <div class="form-group" v-if="(observation.spreadType=='Value')">
                            <label class="col-sm-4 control-label">Spread value</label>
                            <div class="col-sm-8 ">
                                <input type="number" class="form-control" v-model="observation.spreadValue" step="1" min="0" max="99999" />
                            </div>
                        </div>
                        <div class="form-group" v-if="(observation.spreadType=='Percentage')">
                            <label class="col-sm-4 control-label">Spread percentage</label>
                            <div class="col-sm-8 ">
                                <input type="number" class="form-control" v-model="observation.spreadPercentage" step="0.01" min="0.01" max="0.6" />
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-4 control-label">Spread minimum volume</label>
                            <div class="col-sm-8 ">
                                <input type="number" class="form-control" v-model="observation.minimumVolume" step="0.01" min="0.0001" max="99999" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Per volume</label>
                            <div class="col-sm-8 ">
                                <input type="number" class="form-control" v-model="observation.perVolume" step="0.01" min="0.0001" max="99999" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Max volume</label>
                            <div class="col-sm-8 ">
                                <input type="number" class="form-control" v-model="observation.maximumVolume" step="0.01" min="0.0001" max="99999" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4 control-label">Availabe volume</label>
                            <div class="col-sm-8 ">
                                <input type="number" class="form-control" v-model="observation.availabeVolume" step="0.01" min="0.0001" max="99999" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary" v-on:click="save">Save changes</button>
                    </div>
                </div><!-- /.modal-content -->
            </div><!-- /.modal-dialog -->
        </div><!-- /.modal -->
    </div>
</template>
<script>
    var getData = async function () {
        let respone = await this.$http.get('api/observations');
        this.items = respone.data;
    };

    export default {
        data() {
            return {
                items: null,
                observation: null,
                updating: false,
                exchangeNames: null,
        
                currencyPairs: null,
                runningStatus: null,
            }
        },
        methods: {
            add: function () {
                this.updating = false;
                this.observation = { fromExchangeName: "bitstamp", toExchangeName: "gdax", currencyPair: "LtcEur", spreadValue: 30, spreadPercentage: 0.03, spreadType: 'Value', minimumVolume: 0.02, perVolume: 0.01, maximumVolume: 10, availabeVolume: 10, runningStatus: 'Stoped' };
                $('#observationModal').modal('show');
            },
            edit: function (item) {
                this.updating = true;
                this.observation = item;
                $('#observationModal').modal('show');

            },
            remove: async function (item) {
                if (confirm('Are you sure to delete it!')) {
                    await this.$http.delete('api/observations/' + item.id);
                    await getData.call(this);
                }
            },
            save: async function () {
                if (this.updating) {
                    await this.$http.put('api/observations', this.observation);
                }
                else {
                    await this.$http.post('api/observations', this.observation);
                }
                this.$toastr.s("Save successfully.")
                $('#observationModal').modal('hide');
                await getData.call(this);
            },

        },
        computed: {
            title: function () {
                return this.updating ? "Editing" : "Add a new observation";
            }
        },
        async created() {
            try {
                this.exchangeNames = ["gdax", "bitstamp"];
                this.currencyPairs = ["BtcEur", "LtcEur"];
                this.runningStatus = ["Stoped", "Running", "Error", "Done"];
                await getData.call(this);
            } catch (error) {
                console.log(error);
            }
        }

    }

</script>
