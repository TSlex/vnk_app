<template>
  <v-row justify="center" class="text-center">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Создать тип атрибута</span>
          </v-card-title>
          <v-card-text>
            <v-container> </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click.stop="onCancel()"
              >Отмена</v-btn
            >
            <v-btn color="blue darken-1" text type="submit">Создать</v-btn>
            <v-spacer></v-spacer>
          </v-card-actions>
        </v-card>
      </v-form>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import { DataType } from "~/types/Enums/DataType";
import CustomValueField from "~/components/common/CustomValueField.vue";
import { AttributeTypePostDTO } from "~/types/AttributeTypeDTO";

@Component({
  components: {
    CustomValueField,
  },
})
export default class AttributeTypesEdit extends Vue {
  model: AttributeTypePostDTO = {
    name: "",
    defaultCustomValue: "",
    dataType: DataType.String,
    usesDefinedValues: false,
    usesDefinedUnits: false,
    defaultValueIndex: 0,
    defaultUnitIndex: 0,
    values: [],
    units: [],
  };

  showError = false;

  id!: number

  async asyncData({ params }: any) {
    return {id: params.id}
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributeTypesStore.createAttributeType(this.model).then((suceeded) => {
        if (suceeded) {
          this.onCancel();
        } else {
          this.showError = true;
        }
      });
    }
  }

  mounted(){
    console.log(this.id);
  }
}
</script>
