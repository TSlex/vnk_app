import { Store } from 'vuex'
import { getModule } from 'vuex-module-decorators'

import attributes from '@/store/attributes'
import attributeTypes from '@/store/attributetypes'
import identity from '@/store/identity'
import users from '@/store/users'
import orders from '@/store/orders'
import templates from '@/store/templates'
import common from '@/store/common'

let attributesStore: attributes
let attributeTypesStore: attributeTypes
let identityStore: identity
let usersStore: users
let ordersStore: orders
let templatesStore: templates
let commonStore: common

function initialiseStores(store: Store<any>): void {
  attributesStore = getModule(attributes, store)
  attributeTypesStore = getModule(attributeTypes, store)
  identityStore = getModule(identity, store)
  usersStore = getModule(users, store)
  ordersStore = getModule(orders, store)
  templatesStore = getModule(templates, store)
  commonStore = getModule(common, store)
}

export {
  initialiseStores,
  attributesStore,
  attributeTypesStore,
  identityStore,
  usersStore,
  ordersStore,
  templatesStore,
  commonStore
}
