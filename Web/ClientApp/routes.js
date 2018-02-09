import CounterExample from 'components/counter-example'
import HomePage from 'components/home-page'
import Observations from 'components/observations.vue'
import Exchanges from 'components/exchanges.vue'

export const routes = [
    { path: '/', component: HomePage, display: 'Home', style: 'glyphicon glyphicon-home' },
    { path: '/observations', component: Observations, display: 'Observations', style: 'glyphicon glyphicon-eye-open' },
    { path: '/exchanges', component: Exchanges, display: 'Exchanges', style: 'glyphicon glyphicon-euro' },
    { path: '/counter', component: CounterExample, display: 'Counter', style: 'glyphicon glyphicon-education' },
]
