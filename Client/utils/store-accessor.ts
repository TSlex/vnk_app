import { Store } from 'vuex'
import { getModule } from 'vuex-module-decorators'

import attributeTypes from '@/store/attributetypes'
import identity from '@/store/identity'

let attributeTypesStore: attributeTypes
let identityStore: identity

function initialiseStores(store: Store<any>): void {
  attributeTypesStore = getModule(attributeTypes, store)
}

export {
  initialiseStores,
  attributeTypesStore,
  identityStore
}
