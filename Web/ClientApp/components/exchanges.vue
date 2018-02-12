<template>
    <div class="container-fluid">
        <div class="row page-header">
            <div class="row-lg-12"> <strong>Exchanges</strong></div>
        </div>
        <div class="row">
            <div class="row-lg-12 box">
                <p v-if="!exchanges"><em>Loading...</em></p>
                <div v-if="exchanges">
                    <ul class="nav nav-tabs" role="tablist">
                        <li v-for="item in exchanges" v-bind:class="{active:item.show}"><a href="#" v-on:click="active(item)">{{item.name}}</a></li>
                    </ul>
                    <div class="tab-content">
                        <div class="tab-pane" v-for="item in exchanges" v-bind:class="{active:item.show}">

                            <div class="form-group">
                                <label>Settings </label>
                                <textarea class="form-control json" v-model="item.settings"></textarea>
                            </div>
                            <div class="form-group">
                                <button class="btn btn-primary" v-on:click="save(item)">Save</button>
                                <button class="btn btn-default" v-on:click="test(item)">Create a test order</button>
                            </div>
                        </div>
                    </div>
                </div>
                <trade-test ref="test"></trade-test>
            </div>
        </div>
    </div>
</template>

<script>
    import Vue from 'Vue'
    import TradeTest from './trade-test.vue'


    var Exchange = function (name, settings) {
        var self = this;
        self.name = name;
        self.settings = settings;
        self.show = false;
    }

    export default {
        data() {
            return {
                exchanges: null
            }
        },
        components: {
            'trade-test': TradeTest
        },
        methods: {
            test: function (item) {
                this.$refs.test.show(item.name);
            },
            active: function (exchange) {
                this.exchanges.forEach(function (item) {
                    item.show = false;
                })
                exchange.show = true
            },
            save: async function (exchange) {
                let response = await this.$http.put('api/exchanges/settings', exchange);
                if (response.data.isSuccessful) {
                    this.$toastr.s("save successfully");
                }
                else {
                    this.$toastr.e(response.data.message);
                }
            }
        },
        async created() {
            let response = await this.$http.get('api/exchanges/settings');
            var exchanges = [];
            response.data.forEach(function (item) {
                exchanges.push(new Exchange(item.name, item.settings));
            });
            exchanges[0].show = true;
            this.exchanges = exchanges;
        },
    }
</script>
<style>
    textarea.json {
        height: 200px;
    }
</style>
