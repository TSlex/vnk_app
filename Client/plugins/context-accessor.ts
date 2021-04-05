import { Plugin } from '@nuxt/types'
import { connectContext } from '~/utils/vue-context'

const accessor: Plugin = ($ctx) => {
  connectContext($ctx)
}

export default accessor
