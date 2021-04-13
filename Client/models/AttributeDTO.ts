export interface AttributeGetDTO {
  id: number;
  name: string;
  type: string;
  dataType: number;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
}

export interface AttributeDetailsGetDTO {
  id: number;
  name: string;
  type: string;
  defaultValue: string;
  defaultUnit: string;
  usedCount: number;
  dataType: number;
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
