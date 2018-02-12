<template>
    <div class="row">
        <ul class="watch" v-if="items">
            <li v-for="item in items">

                <span>
                    Buy:gdax
                </span>
                <span>
                    Sell:bitstamp
                </span>
                <span>
                    <a href="#" v-if="item.runningStatus=='Running'" v-on:click="updateStatus(item,'Stoped')"><em class="glyphicon glyphicon-stop"></em></a>
                    <a href="#" v-if="item.runningStatus=='Stoped'" v-on:click="updateStatus(item,'Running')"><em class="glyphicon glyphicon-play"></em></a>
                </span>
            </li>
        </ul>
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
            await getData.call(this);
        }
    }
</script>
<style>
    .watch {
        width: 100%;
        list-style: none;
    }

        .watch li {
            border-radius: 2px;
            float: left;
            width: 100px;
            height: 200px;
        }
</style>
