import { DataType } from '~/models/Enums/DataType';

export interface AttributeGetDTO {
  id: number;
  name: string;
  type: string;
  typeId: number;
  dataType: DataType;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
}

export interface AttributeDetailsGetDTO {
  id: number;
  name: string;
  type: string;
  typeId: number;
  defaultValue: string;
  defaultUnit: string | null;
  usedCount: number;
  dataType: DataType;
}

export interface AttributePostDTO {
  name: string;
  attributeTypeId: number;
}

export interface AttributePatchDTO {
  id: number;
  name: string;
  attributeTypeId: number;
}
