import { Store } from 'vuex'
import { getModule } from 'vuex-module-decorators'

import attributeTypes from '@/store/attributetypes'

let attributeTypesStore: attributeTypes

function initialiseStores(store: Store<any>): void {
  attributeTypesStore = getModule(attributeTypes, store)
}

export { initialiseStores, attributeTypesStore }
