<template>
    <v-main class="grey lighten-3">
      <v-container class="text-center" fluid>
        <br>
        <br>
        <br>
        <template v-if="error.statusCode === 404">
          <h1><v-icon class="text-h1 red--text accent-2">mdi-alert-octagon</v-icon></h1>
          <h1 class="text-h3">{{ pageNotFound }}!</h1>
          <h3 class="text-h6 mt-4">Проверьте введеный url. <br>Если ошибка не исчезла, свяжитесь с разработчиком</h3>
        </template>
        <template v-else>
          <h1><v-icon class="text-h1 red--text accent-2">mdi-bug</v-icon></h1>
          <h1 class="text-h3">{{ otherError }}!</h1>
          <h3 class="text-h6 mt-4">Пожалуйста свяжитесь с разработчиком</h3>
        </template>
        <div class="mt-4"><a href="mailto:alex10119996@gmail.com" class="text-body-1">alex10119996@gmail.com</a></div>
      </v-container>
    </v-main>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";

@Component({})
export default class ErrorLayout extends Vue {
  @Prop({ default: null, type: Object })
  error!: Object;

  pageNotFound = "Страница не найдена";
  otherError = "Системная ошибка";

  head() {
    const title =
      (this.error as any).statusCode === 404
        ? this.pageNotFound
        : this.otherError;
    return {
      title,
    };
  }
}
</script>
