<template>
    <div class="row">
        <div class="col-lg-12">
            <ul class="watch" v-if="items">
                <li v-for="item in items">
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Value</th>
                                <th>Name</th>
                                <th>Value</th>
                            </tr>

                        </thead>
                        <tbody>
                            <tr>
                                <td>From exchange</td>
                                <td>{{item.observation.fromExchangeName}}</td>
                                <td>Spread Type</td>
                                <td>{{item.observation.spreadType}}: {{item.observation.spreadType=='Value'?item.observation.spreadValue:item.observation.spreadPercentage}} </td>


                            </tr>
                            <tr>
                                <td>To exchange</td>
                                <td>{{item.observation.toExchangeName}}</td>
                                <td>Per volume</td>
                                <td>{{item.observation.perVolume}}</td>


                            </tr>
                            <tr>
                                <td> Current status: </td>
                                <td>
                                    <strong>{{item.observation.runningStatus}}</strong>  <a href="#" title="click to stop" v-if="item.observation.runningStatus=='Running'" v-on:click="updateStatus(item.observation,'Stoped')"><em class="glyphicon glyphicon-stop"></em></a>
                                    <a href="#" title="click to run" v-if="item.observation.runningStatus=='Stoped'" v-on:click="updateStatus(item.observation,'Running')"><em class="glyphicon glyphicon-play"></em></a>
                                </td>
                                <td>Minimum volume</td>
                                <td>{{item.observation.minimumVolume}}</td>


                            </tr>

                            <tr>
                                <td>Currency pair</td>
                                <td>{{item.observation.currencyPair}}</td>
                                <td>Maximum volume </td>
                                <td>{{item.observation.maximumVolume}} -- {{item.observation.availabeVolume}}</td>
                            </tr>
                            <tr>
                                <td>Ask 1</td>
                                <td>{{item.orderBook.ask1}}</td>
                                <td>Bid 1</td>
                                <td>{{item.orderBook.bid1}}</td>

                            </tr>
                            <tr>
                                <td>Spread Value</td>
                                <td>{{item.orderBook.spreadValue}}</td>
                                <td>Spread Volume</td>
                                <td>{{item.orderBook.spreadVolume}}</td>
                            </tr>
                            <tr>
                                <td>Ask 2</td>
                                <td>{{item.orderBook.ask2}}</td>
                                <td>Bid 2</td>
                                <td>{{item.orderBook.bid2}}</td>
                            </tr>
                            <tr>
                                <td>Ask 3</td>
                                <td>{{item.orderBook.ask3}}</td>
                                <td>Bid 3</td>
                                <td>{{item.orderBook.bid3}}</td>
                            </tr>
                        </tbody>
                    </table>

                </li>
            </ul>
        </div>
    </div>
</template>
<script>

    var times = 0;
    var getData = async function (vue) {
        try {

            let respone = await vue.$http.get('api/watchs');
            vue.items = respone.data;
            times++;
            // console.log(times);
            // console.log(vue.items);

        } catch (ex) {
            //console.log(ex);
        }
    };

    export default {
        data() {
            return {
                items: null,
            };
        },
        methods: {
            updateStatus: async function (item, status) {
               
                let url = `api/observations/${item.id}`;
                let response = await this.$http.put(url, { status: status });
                let result = response.data;
                if (result.isSuccessful) {
                    item.runningStatus = status;
                    this.$toastr.s(result.message);
                }
                else {
                    this.$toastr.e(result.message)
                }
            }
        },
        async created() {
            var self = this;
            getData(self);
            this.task = window.setInterval(function () { getData(self) }, 1000);
        },
        beforeDestroy() {
            console.log('before destory.');
            window.clearInterval(this.task);
        }
    }
</script>
<style>
    .watch {
        width: 100%;
        list-style: none;
        padding-left: 0px;
        overflow-x: auto;
        overflow-y: hidden;
    }

        .watch th {
            width: 25%
        }

        .watch li {
            border-radius: 2px;
            float: left;
            width: 550px;
            height: 350px;
            background-color: white;
            padding: 5px 5px 5px 5px;
            margin-right: 30px;
            margin-bottom: 30px;
        }

            .watch li span {
                display: inline-block;
                width: 200px;
            }
</style>
