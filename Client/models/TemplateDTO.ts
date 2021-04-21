import { DataType } from '~/models/Enums/DataType';

export interface TemplateGetDTO {
  name: string;
  attributes: TemplateAttributeGetDTO[];
  id: number;
}

export interface TemplateAttributeGetDTO {
  attributeId: number;
  typeId: number;
  name: string;
  type: string;
  dataType: DataType;
  featured: boolean;
  id: number;
  usesDefinedValues: boolean,
  usesDefinedUnits: boolean,
}

export interface TemplatePostDTO {
  name: string;
  attributes: TemplateAttributePostDTO[];
}

export interface TemplateAttributePostDTO {
  attributeId: number;
  featured: boolean;
}

export interface TemplatePatchDTO {
  id: number;
  name: string;
}

export interface TemplateAttributePatchDTO {
  id: number;
  featured: boolean;
  attributeId: number;
}
