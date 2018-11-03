﻿using System;
using Asn1;
using System.Text;

namespace Rubeus
{
    public class Ticket
    {
        //Ticket::= [APPLICATION 1] SEQUENCE {
        //        tkt-vno[0] INTEGER(5),
        //        realm[1] Realm,
        //        sname[2] PrincipalName,
        //        enc-part[3] EncryptedData -- EncTicketPart
        //}

        public Ticket(AsnElt body)
        {
            foreach (AsnElt s in body.EnumerateElements()) {
                AsnElt firstItem = s.FirstElement;
                switch (s.TagValue) {
                    case 0:
                        tkt_vno = Convert.ToInt32(firstItem.GetInteger());
                        break;
                    case 1:
                        realm = Encoding.ASCII.GetString(firstItem.GetOctetString());
                        break;
                    case 2:
                        sname = new PrincipalName(firstItem);
                        break;
                    case 3:
                        enc_part = new EncryptedData(firstItem);
                        break;
                    default:
                        break;
                }
            }
        }

        public AsnElt Encode()
        {
            // tkt-vno         [0] INTEGER (5)
            AsnElt tkt_vnoAsn = AsnElt.MakeInteger(tkt_vno);
            AsnElt tkt_vnoSeq = AsnElt.MakeSequence(new AsnElt[] { tkt_vnoAsn });
            tkt_vnoSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, tkt_vnoSeq);


            // realm           [1] Realm
            AsnElt realmAsn = AsnElt.MakeString(AsnElt.IA5String, realm);
            realmAsn = AsnElt.MakeImplicit(AsnElt.UNIVERSAL, AsnElt.GeneralString, realmAsn);
            AsnElt realmAsnSeq = AsnElt.MakeSequence(realmAsn);
            realmAsnSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, realmAsnSeq);


            // sname           [2] PrincipalName
            AsnElt snameAsn = sname.Encode();
            snameAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, snameAsn);


            // enc-part        [3] EncryptedData -- EncTicketPart
            AsnElt enc_partAsn = enc_part.Encode();
            AsnElt enc_partSeq = AsnElt.MakeSequence(enc_partAsn);
            enc_partSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 3, enc_partSeq);


            AsnElt totalSeq = AsnElt.MakeSequence(new[] { tkt_vnoSeq, realmAsnSeq, snameAsn, enc_partSeq });
            AsnElt totalSeq2 = AsnElt.MakeSequence(new[] { totalSeq });
            totalSeq2 = AsnElt.MakeImplicit(AsnElt.APPLICATION, 1, totalSeq2);

            return totalSeq2;
        }

        public int tkt_vno { get; set; }

        public string realm { get; set; }

        public PrincipalName sname { get; set; }

        public EncryptedData enc_part { get; set; }
    }
}