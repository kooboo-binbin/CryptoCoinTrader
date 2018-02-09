<template>
    <div id="tradeModal" class="modal fade" tabindex="-1" role="dialog">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">trade test</h4>
                </div>
                <div class="modal-body form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Buy exchange name</label>
                        <div class="col-sm-8 ">
                            <select class="form-control" v-model="exchangeName">
                                <option v-for="option in exchangeNames">
                                    {{option}}
                                </option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-4 control-label">Currency pair</label>
                        <div class="col-sm-8 ">
                            <select class="form-control" v-model="currencyPair">
                                <option v-for="option in currencyPairs">
                                    {{option}}
                                </option>
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-4 control-label">Price</label>
                        <div class="col-sm-8 ">
                            <input type="number" class="form-control" v-model="price" step="0.01" min="0.0001" max="99999" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Volume</label>
                        <div class="col-sm-8 ">
                            <input type="number" class="form-control" v-model="volume" step="0.01" min="0.0001" max="99999" />
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-dismiss="modal" v-on:click="buy">Buy</button>
                    <button type="button" class="btn btn-default" v-on:click="sell">Sell</button>
                </div>
            </div><!-- /.modal-content -->
        </div><!-- /.modal-dialog -->
    </div><!-- /.modal -->
</template>
<script>
    export default {

        data() {
            return {
                exchangeName: null,
                exchangeNames: null,
                currencyPairs: null,
                currencyPair: null,
                price: 1,
                volume: 1,
            }
        },
        methods: {
            changeName: function (name) {
                this.exchangeName = name;
            },
            buy: async function () {

                var confirmMessage = `Are you sure
price:${this.price}
volume:${this.volume}`;

                if (confirm(confirmMessage)) {
                    
                }
            },
            sell: function () {
                this.$toastr.e("the function is not implemented");
            }
        },
        async created() {
            this.exchangeNames = ["gdax", "bitstamp"];
            this.currencyPairs = ["BtcEur", "LtcEur"];
            this.currencyPair = this.currencyPairs[0];
        }
    }
</script>
