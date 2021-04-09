<template>
  <v-row justify="center" class="text-center" v-if="attributeType">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Изменить тип атрибута</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-alert dense text type="error" v-if="showError">
                {{ error }}
              </v-alert>
              <v-text-field
                label="Название"
                required
                :rules="rules.name"
                v-model="model.name"
              ></v-text-field>
              <v-select
                label="Тип данных"
                required
                :items="dataTypes"
                v-model="model.dataType"
                :rules="rules.type"
              ></v-select>
              <CustomValueField
                :dataType="model.dataType"
                v-model="model.defaultCustomValue"
                :label="`Значение по умолчанию`"
                v-if="!model.usesDefinedValues"
              />
              <template v-if="attributeType.usesDefinedValues">
                <v-toolbar flat>
                  <v-btn text outlined large @click="valueDialog = true"
                    >Добавить</v-btn
                  >
                  <v-spacer></v-spacer>
                  <v-toolbar-title>Допустимые значения</v-toolbar-title>
                </v-toolbar>
                <v-divider></v-divider>
                <template v-if="valuesCount == 0">
                  <div class="pt-2" @click="valueDialog = true">
                    <a>Ничего не добавлено</a>
                  </div>
                </template>
                <div
                  class="d-flex justify-space-between pa-2"
                  v-for="(v, i) in model.values"
                  :key="'value' + v + i"
                >
                  <template>
                    <span v-if="isDateFormat" class="text-body-1">{{
                      v | formatDate
                    }}</span>
                    <span v-else-if="isDateTimeFormat" class="text-body-1">{{
                      v | formatDateTime
                    }}</span>
                    <span v-else class="text-body-1">{{ v }}</span>
                    <span>
                      <v-icon @click="featureValue(i)"
                        >mdi-star{{
                          model.defaultValueIndex === i ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon @click="changeValue(i)">mdi-lead-pencil</v-icon>
                      <v-icon @click="removeValue(i)">mdi-delete</v-icon>
                    </span>
                  </template>
                </div>
                <v-input :rules="rules.values" v-model="model.values"></v-input>
              </template>
              <template v-if="attributeType.usesDefinedUnits">
                <v-toolbar flat>
                  <v-btn text outlined large @click="unitDialog = true"
                    >Добавить</v-btn
                  >
                  <v-spacer></v-spacer>
                  <v-toolbar-title>Единицы измерения</v-toolbar-title>
                </v-toolbar>
                <v-divider></v-divider>
                <template v-if="unitsCount == 0">
                  <div class="pt-2" @click="unitDialog = true">
                    <a>Ничего не добавлено</a>
                  </div>
                </template>
                <div
                  class="d-flex justify-space-between pa-2"
                  v-for="(u, i) in model.units"
                  :key="'unit' + u + i"
                >
                  <template>
                    <span class="text-body-1">{{ u }}</span>
                    <span>
                      <v-icon @click="featureUnit(i)"
                        >mdi-star{{
                          model.defaultUnitIndex === i ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon @click="changeUnit(i)">mdi-lead-pencil</v-icon>
                      <v-icon @click="removeUnit(i)">mdi-delete</v-icon>
                    </span>
                  </template>
                </div>
                <v-input :rules="rules.units" v-model="model.units"></v-input>
              </template>
            </v-container>
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
import { AttributeTypePatchDTO } from "~/types/AttributeTypeDTO";
import { notEmpty, required } from "~/utils/form-validation";
import { localize } from "~/utils/localizeDataType";

@Component({
  components: {
    CustomValueField,
  },
})
export default class AttributeTypesEdit extends Vue {
  model: AttributeTypePatchDTO = {
    id: 0,
    name: "",
    defaultCustomValue: "",
    dataType: DataType.String,
    defaultValueId: 0,
    defaultUnitId: 0,
  };

  showError = false;

  id!: number;

  rules = {
    name: [required()],
    type: [required()],
    values: [notEmpty()],
    units: [notEmpty()],
  };

  get attributeType() {
    return attributeTypesStore.selectedAttributeType;
  }

  get dataTypes() {
    let types: any = [];

    Object.values(DataType).forEach((element) => {
      let key = Number(element);
      if (!isNaN(Number(element)) && key >= 0) {
        types.push({
          text: localize(element as DataType),
          value: element as DataType,
        });
      }
    });

    return types;
  }

  get valuesCount() {
    return this.attributeType?.values?.length ?? 0;
  }

  get unitsCount() {
    return this.attributeType?.units?.length ?? 0;
  }


  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributeTypesStore.updateAttributeType(this.model).then((suceeded) => {
        if (suceeded) {
          this.onCancel();
        } else {
          this.showError = true;
        }
      });
    }
  }

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  mounted() {
    if (!this.id) {
      this.$router.back();
    }

    attributeTypesStore.getAttributeType(this.id).then((suceeded) => {
      if (!suceeded) {
        this.$router.back();
      } else {
        this.model = { ...this.attributeType } as AttributeTypePatchDTO;
      }
    });
  }
}
</script>
