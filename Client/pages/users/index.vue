<template>
  <v-row justify="center" class="text-center">
    <v-col cols="5" class="mt-4">
      <v-sheet class="px-3" rounded="lg">
        <v-row justify="center" class="text-center">
          <v-col cols="4">
            <v-navigation-drawer permanent>
              <v-list dense nav>
                <v-list-item
                  link
                  v-for="(nav, i) in NavTabs"
                  :key="i"
                  @click="setTab(nav.tabIndex)"
                >
                  <v-list-item-icon>
                    <v-icon>{{ nav.icon }}</v-icon>
                  </v-list-item-icon>
                  <v-list-item-content>
                    <v-list-item-title>{{ nav.title }}</v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </v-list>
            </v-navigation-drawer>
          </v-col>
          <template v-if="userSelected"><UserData/></template>
          <template v-else-if="tabIndex == 1"><UsersList /></template>
          <template v-else><UserData /></template>
        </v-row>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

import UserData from "~/components/users/UserData.vue";
import UsersList from "@/components/users/UsersList.vue";
import UserForm from "~/components/users/UserDataDialog.vue";
import { identityStore, usersStore } from "~/store";

@Component({
  components: {
    UserData,
    UsersList,
    UserForm,
  },
})
export default class UsersIndex extends Vue {
  tabIndex = 0;

  navs: { icon: string; title: string; tabIndex: number, condition: () => boolean }[] = [
    {
      icon: "mdi-card-account-details-outline",
      title: "Мои данные",
      tabIndex: 0,
      condition: () => true
    },
    { icon: "mdi-account-multiple", title: "Пользователи", tabIndex: 1, condition: () => this.isAdminOrRoot},
  ];

  get NavTabs(){
    return this.navs.filter(nav => nav.condition())
  }

  get userSelected(){
    return usersStore.selectedUser && usersStore.selectedUser.id != identityStore.userData?.id
  }

  get isAdminOrRoot(){
    return identityStore.isAdministrator || identityStore.isRoot;
  }

  setTab(index: number) {
    this.tabIndex = index;
    usersStore.SELECTED_USER_CLEARED();
  }
}
</script>

