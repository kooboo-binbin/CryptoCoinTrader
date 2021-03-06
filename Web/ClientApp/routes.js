import CounterExample from 'components/counter-example'
import HomePage from 'components/home-page'
import Observations from 'components/observations.vue'
import Exchanges from 'components/exchanges.vue'
import Arbitrage from 'components/arbitrages.vue'
import Orders from 'components/orders.vue'
import Logs from 'components/logs.vue'

export const routes = [
    { path: '/', component: HomePage, display: 'Home', style: 'glyphicon glyphicon-home' },
    { path: '/observations', component: Observations, display: 'Observations', style: 'glyphicon glyphicon-eye-open' },
    { path: '/exchanges', component: Exchanges, display: 'Exchanges', style: 'glyphicon glyphicon-euro' },
    { path: '/arbitrages', component: Arbitrage, display: 'Arbitrages', style: 'glyphicon glyphicon-grain' },
    { path: '/orders', component: Orders, name: 'orders', display: 'Orders', style: 'glyphicon glyphicon-tint' },
    { path: '/logs', component: Logs, display: 'Logs', style:'glyphicon glyphicon-info-sign'}
    //{ path: '/counter', component: CounterExample, display: 'Counter', style: 'glyphicon glyphicon-education' },
]
