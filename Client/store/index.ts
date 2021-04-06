import { Store } from 'vuex'
import { initialiseStores } from '~/utils/store-accessor'
import { config } from 'vuex-module-decorators'

const initializer = (store: Store<any>) => initialiseStores(store)

// Set rawError to true by default on all @Action decorators
config.rawError = true

export const plugins = [initializer]

export * from '~/utils/store-accessor'
